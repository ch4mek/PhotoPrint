import axios from 'axios';

import { GET_USERS_API_ENDPOINT } from '@/constants/queryPaths';
import { InitialAuthState } from '@/context/types';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { UserInfo } from '@/types/userInfo';

import { UseUserInfoParams } from './types';

export const GET_USER_INFO_QUERY_KEY = 'user-info';

/**
 * Hook requests the data about current user and saves it a local storage
 */
export const useUserInfo = (
	persistFunc: (authState: InitialAuthState) => void,
	{ ...options }: UseUserInfoParams
) =>
	useQueryWithCallback<UserInfo>({
		...options,
		queryKey: [GET_USER_INFO_QUERY_KEY],
		queryFn: async () => {
			try {
				const { data } = await axios.get(`${GET_USERS_API_ENDPOINT}/current`);
				persistFunc({ ...data, role: data.position });

				return data;
			} catch (error) {
				return undefined;
			}
		},
		refetchOnWindowFocus: false,
	});
