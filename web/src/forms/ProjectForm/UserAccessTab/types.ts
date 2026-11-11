import { GridCellDefaultProps } from '@/components/Grid';
import { ProjectEntity } from '@/types/project';

export interface UserAccessTabProps {
	projectData: ProjectEntity;
}

export interface UserAccessRow extends GridCellDefaultProps {
	userFullName: string;
	isTimeOnProjectBillable: string;
	isActive: boolean;
	userOnProjectFrom: string;
	userOnProjectTo: string | null;
}
