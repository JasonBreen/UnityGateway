import json
import urllib.error
from unittest.mock import MagicMock, patch
from Tools.osv_scan import query_osv

@patch("urllib.request.urlopen")
def test_query_osv_json_decode_error(mock_urlopen):
    # Setup the mock to return a response that raises JSONDecodeError when json.load is called
    mock_response = MagicMock()
    # json.load calls .read() under the hood, or we can just mock json.load directly
    # A cleaner way is to mock the read method to return invalid JSON
    mock_response.read.return_value = b"invalid json"

    # URLOpen acts as a context manager returning the response object
    mock_urlopen.return_value.__enter__.return_value = mock_response

    payload = {"package": {"name": "test", "ecosystem": "test"}, "version": "1.0.0"}

    result = query_osv(payload)

    assert result["status"] == "error"
    assert "Invalid JSON response from OSV API" in result["error"]
    assert result["payload"] == payload

@patch("urllib.request.urlopen")
def test_query_osv_http_error(mock_urlopen):
    mock_urlopen.side_effect = urllib.error.HTTPError("url", 500, "Internal Server Error", {}, None)

    payload = {"package": {"name": "test", "ecosystem": "test"}, "version": "1.0.0"}

    result = query_osv(payload)

    assert result["status"] == "error"
    assert "HTTP 500 while querying OSV: Internal Server Error" in result["error"]
    assert result["payload"] == payload

@patch("urllib.request.urlopen")
def test_query_osv_success(mock_urlopen):
    mock_response = MagicMock()
    # Need to mock what json.load reads.
    # The easiest way is to mock json.load to return what we want, but it's better to provide valid json string
    # since json.load accepts a file-like object and calls .read() on it
    import io
    mock_response = io.BytesIO(b'{"vulns": [{"id": "CVE-2023-1234"}]}')

    mock_urlopen.return_value.__enter__.return_value = mock_response

    payload = {"package": {"name": "test", "ecosystem": "test"}, "version": "1.0.0"}
    result = query_osv(payload)

    assert result["status"] == "ok"
    assert result["vulnerabilities"] == [{"id": "CVE-2023-1234"}]
    assert result["payload"] == payload
