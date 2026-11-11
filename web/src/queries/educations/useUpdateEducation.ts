import { useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { EDUCATIONS_ENDPOINT } from '@/constants/queryPaths';
import { EducationFormValues } from '@/forms/TeamboardForm/Education/types';

import { UseUpdateEducationProps } from './types';

export const useUpdateEducation = ({
	userId,
	educationId,
	options,
}: UseUpdateEducationProps) =>
	useMutation<void, AxiosError<ServerResponseError>, EducationFormValues>({
		...options,
		mutationFn: async ({
			educationLevel,
			studyForm,
			graduationYear,
			comment,
			...otherFields
		}: EducationFormValues) =>
			axios.put(`${EDUCATIONS_ENDPOINT}/${educationId}`, {
				userId,
				educationLevel: educationLevel?.name,
				studyForm: studyForm?.name,
				graduationYear: Number(graduationYear),
				comment: !comment ? null : comment,
				...otherFields,
			}),
	});
