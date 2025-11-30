import { useState, useEffect } from 'react';

export default function useHash(): [string, (newHash: string) => void] {
  const [hash, setHash] = useState('');

  useEffect(() => {
    // Set initial hash value on client side
    setHash(window.location.hash);

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