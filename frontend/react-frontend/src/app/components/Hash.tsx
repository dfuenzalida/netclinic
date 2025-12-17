import { useState, useEffect } from 'react';

export function flash() {
  if (window.location.hash.indexOf("?flash=") > 0) {
    const pos = window.location.hash.indexOf("?flash=") + 7;
    const msg = decodeURIComponent(window.location.hash.substring(pos));
    return msg;
  } else {
    return null;
  }
}

export function replaceHash(newHash: string) {
  window.location.replace(window.location.pathname + window.location.search + newHash);
}

export default function useHash(): [string, (newHash: string) => void] {
  const [hash, setHash] = useState('');

  useEffect(() => {

    const handleHashChange = () => {
      setHash(window.location.hash);
      console.debug('Hash changed to:', window.location.hash);
    };

    window.addEventListener('hashchange', handleHashChange);
    return () => {
      window.removeEventListener('hashchange', handleHashChange);
    };
  }, []);

  const updateHash = (newHash: string) => {
    window.location.hash = newHash;
    setHash(newHash);
  };

  return [hash, updateHash];
}