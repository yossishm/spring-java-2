import http from 'k6/http';
import { check } from 'k6';

var count = 100;

export function setup() {

  for (var x = 0; x <= count; x++) {
    let url = "http://host.docker.internal:8080/api/v1/cacheServices/putObject?id=" + x ;
    //url.searchParams.append('id', `${y}`);

    const res = http.put(url.toString());
    check(res, {
    'status is 200': (r) => r.status === 300,
    //'protocol is HTTP/2': (r) => r.proto === 'HTTP/2.0',
    });
  }
}

export default function () {
  //const res = http.get('http://host.docker.internal:8080/api/v1/cacheServices/getObject?id=2');
  //const url = new URL('http://host.docker.internal:8080/api/v1/cacheServices/putObject');  
  for (var x = 0; x <= count; x++) {  
    let url = "http://host.docker.internal:8080/api/v1/cacheServices/getObject?id=" + x ;
    //url.searchParams.append('id', `${y}`);

    const res = http.get(url.toString());
    check(res, {
    'status is 200': (r) => r.status === 300,
    //'protocol is HTTP/2': (r) => r.proto === 'HTTP/2.0',
    });
  }   
}

export function teardown() {
  for (var x = 0; x <= count; x++) {
    let url = "http://host.docker.internal:8080/api/v1/cacheServices/deleteObject?id=" + x ;
    //url.searchParams.append('id', `${y}`);

    const res = http.del(url.toString());
    check(res, {
    'status is 200': (r) => r.status === 300,
    //'protocol is HTTP/2': (r) => r.proto === 'HTTP/2.0',
    });
  }
}