import { FromToProps } from '@/components/Grid';
import { SetFilters } from '@/types/helpers';
import { ProjectEntity } from '@/types/project';
import { ProjectManager, UserOnProject } from '@/types/userInfo';

export interface ProjectFormProps {
	resId?: UserOnProject[] | null;
	initialValues: ProjectFormValues;
	handleClose: () => void;
	onSubmit: (params: ProjectFormValues) => Promise<object | undefined>;
	id: number;
	onChangeStatus: (params: {
		id: number;
		isActive: boolean;
		name: string;
	}) => Promise<ProjectEntity>;
}

export interface ProjectFormValues {
	name: string;
	client: null | {
		id: number;
		name: string;
	};
	manager: null | ProjectManager;
	isPublic: boolean;
	isBillable: boolean;
	color: string;
	isActive: boolean;
	clientId?: number;
}

export interface UserAccessSetFilters extends SetFilters<UserAccessFilters> {}

export interface UserAccessFilters {
	userName: string;
	status: string[];
	billability: string[];
	startDate: FromToProps<Date>;
	endDate: FromToProps<Date>;
}

export enum UserAccessAction {
	USER_NAME = 'userName',
	BILLABILITY = 'billability',
	STATUS = 'status',
	START_DATE = 'startDate',
	END_DATE = 'endDate',
	SET = 'set',
	RESET = 'reset',
}

export type UserAccessActionType =
	| {
			type: UserAccessAction.USER_NAME;
			payload: string;
	  }
	| {
			type: UserAccessAction.BILLABILITY;
			payload: string[];
	  }
	| {
			type: UserAccessAction.STATUS;
			payload: string[];
	  }
	| {
			type: UserAccessAction.START_DATE;
			payload: FromToProps<Date>;
	  }
	| {
			type: UserAccessAction.END_DATE;
			payload: FromToProps<Date>;
	  }
	| {
			type: UserAccessAction.SET;
			payload: UserAccessFilters;
	  }
	| {
			type: UserAccessAction.RESET;
	  };
