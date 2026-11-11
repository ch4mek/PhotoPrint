import axios from 'axios';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { getAssignedUserSoftSkillsEndpoint } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { AssignedUserSoftSkill } from '@/types/teamboard';

export const useGetAssignedUsersSoftSkills = (
	userId?: number,
	snapshotId?: number | string
) =>
	useQueryWithCallback({
		queryKey: [
			TeamboardQueriesKeys.useGetAssignedUsersSoftSkills,
			userId,
			...(snapshotId ? [snapshotId] : []),
		],
		queryFn: async () => {
			const { data } = await axios.get<AssignedUserSoftSkill[]>(
				getAssignedUserSoftSkillsEndpoint(userId),
				{
					params: {
						snapshotId,
					},
				}
			);

			return data;
		},
		refetchOnWindowFocus: false,

		enabled: !!userId,
	});
