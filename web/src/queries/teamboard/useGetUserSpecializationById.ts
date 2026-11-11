import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { getUserSpecializationById } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { FullHardSkillInfo, UserHardSkillLookup } from '@/types/hardSkills';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetUserSpecializationById = (
	specializationId: number | undefined,
	options?: PatchedQueryOptions<
		UserHardSkillLookup<FullHardSkillInfo>,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<
		UserHardSkillLookup<FullHardSkillInfo>,
		AxiosError<ServerResponseError>
	>({
		queryKey: [
			TeamboardQueriesKeys.useGetUserSpecializationById,
			specializationId,
		],
		queryFn: () =>
			axios
				.get<UserHardSkillLookup<FullHardSkillInfo>>(
					getUserSpecializationById(specializationId)
				)
				.then((res) => res.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});
