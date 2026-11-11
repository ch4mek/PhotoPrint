import { FromToProps } from '@/components/Grid';
import { InterviewFilters } from '@/pages/Staffing/components/InterviewTab/types';
import { InterviewStatus, InterviewType } from '@/types/interview';

export interface InterviewFilterStore extends InterviewFilters {
	initialFilters: InterviewFilters;

	setId: (id: string) => void;
	setUser: (user: string) => void;
	setSpecialization: (specialization: string) => void;
	setClient: (client: string) => void;
	setType: (type: InterviewType[]) => void;
	setDate: (date: FromToProps<Date>) => void;
	setTime: (time: FromToProps<string>) => void;
	setStatus: (status: InterviewStatus[]) => void;
	setSaleManager: (manager: string) => void;
	setJiraTask: (jiraTask: string) => void;
	setReasonForRefusal: (reasongForRefusal: string) => void;
	setNote: (notes: string) => void;

	setAll: (filters: InterviewFilters) => void;
	reset: () => void;

	initialize: (initialFilters: InterviewFilters) => void;
}
