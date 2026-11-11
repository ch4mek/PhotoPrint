import axios, { AxiosError } from 'axios';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_SPECIALIZATIONS_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ShortHardSkillInfo, UserHardSkillLookup } from '@/types/hardSkills';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { ServerResponseError } from './types';

export const useUserSpecializationLookup = (
	userId: string | number | undefined,
	specializationTemplateName: string | undefined,
	options?: PatchedQueryOptions<
		Array<UserHardSkillLookup<ShortHardSkillInfo>>,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<
		Array<UserHardSkillLookup<ShortHardSkillInfo>>,
		AxiosError<ServerResponseError>
	>({
		queryKey: [
			TeamboardQueriesKeys.userSpecializationsLookup,
			userId,
			specializationTemplateName,
		],
		queryFn: () =>
			axios
				.get<Array<UserHardSkillLookup<ShortHardSkillInfo>>>(
					`${GET_USER_SPECIALIZATIONS_ENDPOINT}/lookup`,
					{
						params: {
							userId,
							specializationTemplateName,
						},
					}
				)
				.then((res) => res.data),
		refetchOnWindowFocus: false,
		enabled: !!userId,
		...options,
	});
