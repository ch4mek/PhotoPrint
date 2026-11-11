import { useMutation, useQueryClient } from '@tanstack/react-query';
import axios from 'axios';

import { homeQueriesKeys } from '@/constants/queriesKeys';
import { USER_SCORE_API_ENDPOINT } from '@/constants/queryPaths';
import {
	createProgrammerDaySignature,
	PROGRAMMER_DAY_SIGNATURE_SECRET,
} from '@/helpers/programmerDaySignature';
import { handleRequestError } from '@/helpers/requestErrorHandler';

import { ProgrammerDayScoreRequest } from './types';

export const useSaveProgrammerDayScore = (userId: number) => {
	const queryClient = useQueryClient();

	return useMutation({
		mutationFn: async ({ score }: ProgrammerDayScoreRequest) => {
			try {
				const timestamp = Math.floor(Date.now() / 1000);
				const signature = await createProgrammerDaySignature(
					userId,
					score,
					timestamp,
					PROGRAMMER_DAY_SIGNATURE_SECRET
				);
				const { data } = await axios.put<void>(USER_SCORE_API_ENDPOINT, {
					score,
					timestamp,
					signature,
				});

				return data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		onSuccess: () =>
			queryClient.invalidateQueries({
				queryKey: [homeQueriesKeys.getProgrammerDayResults],
			}),
	});
};
