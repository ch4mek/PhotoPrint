import { useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { departmentGroupUsersEndpoint } from '@/constants/queryPaths';
import { UseUserResponse } from '@/types/userInfo';

import { UseAddUserToGroupProps } from './types';

export const useAddUserToGroup = ({
	groupId,
	options,
}: UseAddUserToGroupProps) =>
	useMutation<UseUserResponse, AxiosError<ServerResponseError>, number>({
		...options,
		mutationFn: async (userId: number) => {
			const { data } = await axios.post<UseUserResponse>(
				`${departmentGroupUsersEndpoint(groupId)}`,
				userId,
				{
					headers: {
						'Content-Type': 'application/json',
					},
				}
			);

			return data;
		},
	});
