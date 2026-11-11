import { keepPreviousData } from '@tanstack/react-query';
import axios from 'axios';
import buildQuery from 'odata-query';

import { SoftSkillsParams, UserSoftSkillsResponse } from '../types';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { getUserSoftSkillsEndpoint } from '@/constants/queryPaths';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';

export const useUserSoftSkills = ({
	order,
	orderBy,
	userId,
	top,
	skip,
	...params
}: SoftSkillsParams) =>
	useQueryWithCallback({
		queryKey: [
			TeamboardQueriesKeys.userSoftSkill,
			order,
			top,
			skip,
			userId,
			params.filters?.softSkillName,
		],
		queryFn: async () => {
			const { filters } = params;

			const { softSkillName } = filters || {};
			const query = buildQuery({
				filter: {
					and: [
						softSkillName &&
							`(contains(tolower(softSkillName), tolower('${replaceSpecialCharacters(
								softSkillName
							)}')))`,
					],
				},
				orderBy: `${orderBy} ${order}`,
				top,
				skip,
			});

			const { data } = await axios.get<UserSoftSkillsResponse>(
				`${getUserSoftSkillsEndpoint(userId)}${query}`
			);

			return data;
		},
		refetchOnWindowFocus: false,
		placeholderData: keepPreviousData,
		enabled: !!userId,
	});
