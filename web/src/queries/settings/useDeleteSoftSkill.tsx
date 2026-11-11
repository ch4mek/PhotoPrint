import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_SOFT_SKILLS_API_ENDPOINT } from '@/constants/queryPaths';
import { SoftSkill } from '@/types/settings';

export const useDeleteSoftSkill = (
	options: UseMutationOptions<SoftSkill, AxiosError, SoftSkill>
) =>
	useMutation({
		mutationFn: async ({ id }: SoftSkill) => {
			try {
				const { data } = await axios.delete(
					`${GET_SOFT_SKILLS_API_ENDPOINT}/${id}`
				);

				return data;
			} catch (error) {
				return Promise.reject(error);
			}
		},
		...options,
	});
