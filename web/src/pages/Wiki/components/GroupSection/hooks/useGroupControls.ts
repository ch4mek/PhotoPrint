import { ChangeEvent, useCallback, useState } from 'react';

import { useDebounce } from '@/hooks/useDebounce';

export const useGroupControls = () => {
	const [searchValue, setSearchValue] = useState('');

	const debouncedSearchValue = useDebounce(searchValue, 300);

	const handleSearchChange = useCallback(
		(event: ChangeEvent<HTMLInputElement>) => {
			setSearchValue(event.target.value);
		},
		[]
	);

	const handleSearchReset = useCallback(() => {
		setSearchValue('');
	}, []);

	return {
		searchValue,
		debouncedSearchValue,
		handleSearchChange,
		handleSearchReset,
	};
};
