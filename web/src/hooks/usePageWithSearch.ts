import { useEffect, useState } from 'react';

import { INITIAL_PAGE_NUMBER } from '@/constants/pagination';

export const usePageWithSearch = () => {
	const [search, setSearch] = useState('');
	const [lastSearch, setLastSearch] = useState('');
	const [pageWithoutSearch, setPageWithoutSearch] = useState<string | number>(
		INITIAL_PAGE_NUMBER
	);
	const [pageWithSearch, setPageWithSearch] = useState<string | number>(
		INITIAL_PAGE_NUMBER
	);

	useEffect(() => {
		if (!search) {
			setPageWithSearch(INITIAL_PAGE_NUMBER);
		}

		if (lastSearch !== search) {
			setLastSearch(search);
			setPageWithSearch(INITIAL_PAGE_NUMBER);
		}
	}, [search]);

	const pageAndSearch = {
		page: search ? +pageWithSearch : +pageWithoutSearch,
		setPage: search ? setPageWithSearch : setPageWithoutSearch,
		search,
		setSearch,
	};

	return pageAndSearch;
};
