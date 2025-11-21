'use client';

import { useState } from 'react';
import { OwnersViewNames } from '../types/Types';
import OwnerSearchForm from './Owners/OwnerSearchForm';
import OwnerSearchResults from './Owners/OwnerSearchResults';
import OwnerDetailsView from './Owners/OwnerDetailsView';
import OwnerCreateForm from './Owners/OwnerCreateForm';

export default function Owners() {

  const [ownerId, setOwnerId] = useState<number>(0);
  const [lastName, setLastName] = useState<string>('');
  const [ownersView, setOwnersView] = useState<OwnersViewNames>('searchForm');
  const [errorMessage, setErrorMessage] = useState<string|null>(null);

  switch (ownersView) {
    case 'ownerDetails':
      return <OwnerDetailsView ownerId={ownerId} setOwnersView={setOwnersView} />;
    case 'searchResults':
      return <OwnerSearchResults lastName={lastName} ownersView={ownersView} setOwnersView={setOwnersView}
                errorMessage={errorMessage} setErrorMessage={setErrorMessage} setOwnerId={setOwnerId} />;
    case 'searchForm':
      return <OwnerSearchForm lastName={lastName} setLastName={setLastName}
                errorMessage={errorMessage} setOwnersView={setOwnersView} />;
    case 'ownerCreateForm':
      return <OwnerCreateForm />;
  };
}