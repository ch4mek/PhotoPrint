import { SortDirection } from 'react-data-grid';

import { InterviewFilters } from '@/pages/Staffing/components/InterviewTab/types';
import { SetFilters } from '@/types/helpers';

export type InterviewTabFilters = Omit<InterviewFilters, 'user' | 'userId'>;

export enum InterviewTabFiltersNames {
	ID = 'id',
	SPECIALIZATION = 'specialization',
	CLIENT = 'client',
	TYPE = 'type',
	DATE = 'date',
	TIME = 'time',
	STATUS = 'status',
	SALE_MANAGER = 'saleManager',
	JIRA_TASK = 'jiraTask',
	REASON_FOR_REFUSAL = 'reasonForRefusal',
	NOTE = 'note',
	RESET = 'reset',
	SET = 'set',
}

export type InterviewAction =
	| {
			[K in keyof InterviewTabFilters]: {
				type: K;
				payload: InterviewTabFilters[K];
			};
	  }[keyof InterviewTabFilters]
	| { type: InterviewTabFiltersNames.RESET }
	| { type: InterviewTabFiltersNames.SET; payload: InterviewTabFilters };

export interface InterviewSetFilters extends SetFilters<InterviewTabFilters> {}

export type SortMapper = (direction: SortDirection) => string;
