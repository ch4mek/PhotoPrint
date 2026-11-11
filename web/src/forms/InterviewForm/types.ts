import { Dayjs } from '@/lib/dayjs';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';
import {
	InterviewData,
	InterviewStatus,
	InterviewType,
} from '@/types/interview';

interface InterviewUser {
	id: number;
	firstName: string;
	lastName: string;
	firstNameRu: string;
	lastNameRu: string;
}

interface PreselectedUser {
	id: number;
	name: string;
}

export interface InterviewFormProps {
	onClose: VoidFunction;
	interview?: InterviewData;
	parentInterview?: InterviewData;
	currentUser?: InterviewUser;
	preselectedUser?: PreselectedUser;
}

export interface InterviewFormValues {
	user: SimpleConfigurableConfigurationItem<number> | null;
	specialization: string;
	client: string;
	jiraTask: string;
	type: SimpleConfigurableConfigurationItem<InterviewType> | null;
	status: SimpleConfigurableConfigurationItem<InterviewStatus> | null;
	date: Dayjs | null;
	time: string;
	saleManager: SimpleConfigurableConfigurationItem<number> | null;
	reasonForRefusal: string;
	note: string;
}

export interface GetInitialValuesParams {
	isSaleManager: boolean;
	saleManagerOption: SimpleConfigurableConfigurationItem<number>;
	interview?: InterviewData;
	parentInterview?: InterviewData;
	preselectedUser?: PreselectedUser;
}
