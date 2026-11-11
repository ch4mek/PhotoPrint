import { useContext } from 'react';

import { PersonalDataContext } from '../../context/personalDataContext';

import { CommonProjectAccessTab } from '@/components/CommonProjectAccessTab';

export function TeamboardProjectAccessTab() {
	const {
		userId,
		data: { fullName },
	} = useContext(PersonalDataContext);

	const user = {
		id: userId!,
		fullName,
	};

	if (user.id === undefined) {
		return null;
	}

	return <CommonProjectAccessTab user={user} applyDefaultSorting />;
}
