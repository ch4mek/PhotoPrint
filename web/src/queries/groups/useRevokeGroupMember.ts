import { useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { departmentGroupUsersEndpoint } from '@/constants/queryPaths';
import { UseUserResponse } from '@/types/userInfo';

import { UseRevokeGroupMemberProps } from './types';

export const useRevokeGroupMember = ({
	groupId,
	options,
}: UseRevokeGroupMemberProps) =>
	useMutation<UseUserResponse, AxiosError<ServerResponseError>, number>({
		...options,
		mutationFn: async (userId: number) =>
			axios.delete(`${departmentGroupUsersEndpoint(groupId, userId)}`),
	});
