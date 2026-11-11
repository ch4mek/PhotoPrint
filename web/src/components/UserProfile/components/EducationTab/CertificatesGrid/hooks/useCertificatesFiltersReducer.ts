import { useMemo, useReducer } from 'react';

import { defaultUserCertificatesFilters } from '../constants';
import { UserCertificatesFiltersReducer } from '../types';
import {
	createUserCertificatesFiltersHandlers,
	userCertificatesReducer,
} from '../utils';

export function useCertificatesFiltersReducer(): UserCertificatesFiltersReducer {
	const [filters, dispatch] = useReducer(
		userCertificatesReducer,
		defaultUserCertificatesFilters
	);

	const setFilters = useMemo(
		() => createUserCertificatesFiltersHandlers(filters, dispatch),
		[filters]
	);

	return [filters, setFilters];
}
