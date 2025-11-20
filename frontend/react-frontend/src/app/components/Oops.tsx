import { useEffect, useState } from "react";

export default function Oops() {

  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const fetchOops = async () => {
    try {
      setLoading(true);
      const response = await fetch(`/api/oops`);
      
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      
    } catch (err) {
      setError(err instanceof Error ? err.message : 'An error occurred');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchOops();
  }, []); // Add currentPage as dependency to refetch when it changes

  if (loading) {
    return (
      <div>
        <p>Loading...</p>
      </div>
    );
  }

  if (error) {
    return (
      <div>
        <img src="images/pets.png" />
          <h2>Something happened...</h2>
          <p></p>
      </div>
    );
  }


  return (
    <div>
      <h2>Oops page</h2>
    </div>
  );
}