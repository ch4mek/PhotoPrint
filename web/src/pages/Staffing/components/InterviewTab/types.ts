import { FromToProps } from '@/components/Grid';
import { SetFilters } from '@/types/helpers';
import {
	InterviewStatus,
	InterviewType,
	LookupSaleManagersResponse,
} from '@/types/interview';
import { LookupUserResponse } from '@/types/userInfo';

export interface InterviewGridRow {
	id: number;
	user: string;
	userId: number;
	specialization: string;
	clientName: string;
	jiraTask: string;
	type: string;
	interviewTypeOrderWeight: number;
	status: string;
	statusText: string;
	statusOrderWeight: number;
	date: string | null;
	time: string | null;
	saleManager: string;
	reasonForRefusal: string;
	note: string;
	isMainInterview: boolean;
	itemRowHeight: number;
	isActive: boolean;
	canViewPersonalInfo: boolean;
	handleEdit: (itemId?: string | number) => () => void;
	handleDelete?: (itemId?: string | number) => () => void;
	handleAddStage?: (itemId?: string | number) => () => void;
	hasStages?: boolean;
	expanded?: boolean;
}

export interface InterviewFilters {
	id: string;
	user: string;
	specialization: string;
	client: string;
	jiraTask: string;
	type: InterviewType[];
	status: InterviewStatus[];
	date: FromToProps<Date>;
	time: FromToProps<string>;
	saleManager: string;
	reasonForRefusal: string;
	note: string;
	userId?: string;
}

export interface InterviewOptions {
	userOptions: string[];
	saleManagerOptions: string[];
}

export interface InterviewSetFilters extends SetFilters<InterviewFilters> {}

export type HandleRowClick = (data: InterviewGridRow) => void;

export interface InterviewGridOptionsProps {
	users?: LookupUserResponse[];
	managers?: LookupSaleManagersResponse[];
}
