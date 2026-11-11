import { QueryClient } from '@tanstack/react-query';

import { GET_USER_INFO_QUERY_KEY } from '@/queries/useUserInfo';
import { UserInfo } from '@/types/userInfo';

export const updateUserCache = async (
	queryClient: QueryClient,
	dataUser: UserInfo
) => {
	await queryClient.setQueryData([GET_USER_INFO_QUERY_KEY], (prev: any) => ({
		...prev,
		...dataUser,
	}));
};
