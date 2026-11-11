import { keepPreviousData } from '@tanstack/react-query';
import axios from 'axios';

import { GET_USERS_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { UseUserResponse } from '@/types/userInfo';

export const useUser = (id: number) =>
	useQueryWithCallback<UseUserResponse>({
		queryKey: ['user', id],
		queryFn: async () =>
			axios
				.get<UseUserResponse>(`${GET_USERS_API_ENDPOINT}/${id}`)
				.then((res) => res.data),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		enabled: !!id,
	});
