import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { NotificationType } from '@/constants/notifications';
import { UsersQueriesKeys } from '@/constants/queriesKeys';
import { GET_USERS_TAB_LOOKUP_API_ENDPOINT } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { LookupUserResponse } from '@/types/userInfo';

export const useUsersTabLookup = (toast: ToastContextValues) =>
	useQueryWithCallback<Array<LookupUserResponse>, AxiosError>({
		queryKey: [UsersQueriesKeys.usersTabLookup],
		queryFn: async () => {
			try {
				const { data } = await axios.get(GET_USERS_TAB_LOOKUP_API_ENDPOINT);

				return data;
			} catch (error: unknown) {
				if ((error as AxiosError)?.message) {
					toast.open(
						createToast(NotificationType.ERROR, (error as AxiosError).message)
					);
				}

				return Promise.reject();
			}
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		retry: false,
	});
