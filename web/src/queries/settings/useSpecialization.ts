import { keepPreviousData } from '@tanstack/react-query';
import axios from 'axios';

import { GET_SPECIALIZATION_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { DetailedSpecialization } from '@/types/settings';

export const useSpecialization = (id: number | string | undefined) =>
	useQueryWithCallback({
		queryKey: ['specialization', id],
		queryFn: () =>
			axios
				.get<DetailedSpecialization>(`${GET_SPECIALIZATION_API_ENDPOINT}/${id}`)
				.then((res) => res.data),

		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		enabled: Boolean(id),
	});
