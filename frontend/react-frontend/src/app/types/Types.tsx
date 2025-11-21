export type PageNames = "vets" | "welcome" | "owners" | "oops";
export type OwnersViewNames = 'ownerDetails' | 'searchResults' | 'searchForm' | 'ownerCreateForm';

// Owner Types

export interface OwnerOverview {
  id: number;
  firstName: string;
  lastName: string;
  address: string;
  city: string;
  telephone: string;
  pets: string[];
}

export interface VisitDetails {
  id: number;
  visitDate: string;
  description: string;
}

export interface PetDetails {
  id: number;
  name: string;
  type: string;
  birthDate: string;
  visits: VisitDetails[];
}

export interface OwnerDetails {
  id: number;
  firstName: string;
  lastName: string;
  address: string;
  city: string;
  telephone: string;
  pets: PetDetails[];
}

export interface OwnerSearchProps {
  lastName: string;
  setLastName: (name: string) => void;
  setOwnersView: (view: OwnersViewNames) => void;
  errorMessage: string | null;
  setErrorMessage: (message: string | null) => void;
}

export interface OwnerSearchResultsProps {
  lastName: string;
  ownersView: OwnersViewNames;
  setOwnersView: (view: OwnersViewNames) => void;
  errorMessage: string | null;
  setErrorMessage: (message: string | null) => void;
  setOwnerId: (id: number) => void;
}

export interface OwnerDetailsProps {
  ownerId: number;
  setOwnersView: (view: OwnersViewNames) => void;
}

// Vets

export interface Vet {
  id: number;
  firstName: string;
  lastName: string;
  specialties: Specialty[];
}

export interface Specialty {
  id: number;
  name: string;
}

// NavBar

export interface NavBarProps {
  currentView: PageNames;
  setCurrentView: (viewName: PageNames) => void;
}


// Pagination

export interface PaginationProps {
  currentPage: number;
  setCurrentPage: (page: number) => void;
  totalPages: number;
}
