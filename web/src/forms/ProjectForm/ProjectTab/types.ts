import { ProjectFormProps } from '../types';

export interface ProjectTabProps
	extends Pick<ProjectFormProps, 'handleClose' | 'onChangeStatus' | 'id'> {
	isActivityFormOpen: boolean;
	closeActivityForm: () => void;
}
