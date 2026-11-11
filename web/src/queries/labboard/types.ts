import { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { SortDirection } from '@/constants/sort';
import { LabboardFilters } from '@/pages/Labboard/types';
import { LabboardInternsResponse } from '@/types/labboard';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export interface UseGetLabboardInternsParams {
	page: number;
	rowsPerPage: number;
	order: SortDirection;
	orderBy: string;
	filters: LabboardFilters;
	englishLevelsOptionsLength: number;
	internshipTypesOptionsLength: number;
	finalResultsOptionsLength: number;
	options?: PatchedQueryOptions<
		LabboardInternsResponse,
		AxiosError<ServerResponseError>
	>;
}
