import { UseMutationOptions } from '@tanstack/react-query';
import { AxiosError } from 'axios';

import { ByPageResponse, ServerResponseError } from '../types';

import { SortDirection } from '@/constants/sort';
import { EducationFormValues } from '@/forms/TeamboardForm/Education/types';
import { PersistentConfigurationItem } from '@/types/configuration';
import { EducationsGridFilters, EducationsResponse } from '@/types/educations';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export type UseGetEducationsProps = {
	userId: number;
	filters: EducationsGridFilters;
	order: SortDirection;
	orderBy: string;
	options?: PatchedQueryOptions<
		ByPageResponse<EducationsResponse>,
		AxiosError<ServerResponseError>
	>;
	page: number;
	rowsPerPage: number;
	allFilterOptions: {
		educationLevels: Array<PersistentConfigurationItem>;
		studyForms: Array<PersistentConfigurationItem>;
	};
};

export type UseAddEducationProps = {
	userId: number;
	options?: UseMutationOptions<
		void,
		AxiosError<ServerResponseError>,
		EducationFormValues
	>;
};

export type UseUpdateEducationProps = {
	userId: number;
	educationId: number;
	options?: UseMutationOptions<
		void,
		AxiosError<ServerResponseError>,
		EducationFormValues
	>;
};
