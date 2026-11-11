import { PlanCompletionStatus } from '../constants';

export enum TypeOfTimePart {
	Main = 'main',
	Secondary = 'secondary',
	Total = 'total',
}

export interface TimePartProps extends BaseWorkloadProps {
	firstTime: number;
	secondTime: number;
	isCurrentDate?: boolean;
	isFutureDate?: boolean;
	disabled?: boolean;
}

export interface BaseWorkloadProps {
	type: TypeOfTimePart;
	disabled?: boolean;
	isSubRow?: boolean;
}

export interface FirstWorkloadPartProps extends BaseWorkloadProps {
	planCompletion: PlanCompletionStatus;
}

export interface SecondWorkloadPartProps extends FirstWorkloadPartProps {
	isPrimaryValue: boolean;
}
