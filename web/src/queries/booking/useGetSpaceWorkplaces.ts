import { keepPreviousData } from '@tanstack/react-query';
import axios from 'axios';

import { BookingQueriesKeys } from '@/constants/queriesKeys';
import { getWorkplacesByOfficeIdAPIEndpoint } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';

import { GetSpaceWorkplacesConfig } from './types';

export const useGetOfficeWorkplaces = (officeId: number) =>
	useQueryWithCallback({
		queryKey: [BookingQueriesKeys.spaceWorkplaces, officeId],
		queryFn: async () => {
			if (officeId) {
				const response = await axios.get<GetSpaceWorkplacesConfig>(
					getWorkplacesByOfficeIdAPIEndpoint(officeId)
				);

				return response.data;
			}

			return undefined;
		},
		placeholderData: keepPreviousData,
		enabled: Boolean(officeId),
		refetchOnWindowFocus: false,
	});
