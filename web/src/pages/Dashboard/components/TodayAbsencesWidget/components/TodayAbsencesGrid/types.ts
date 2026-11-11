import { GridCellDefaultProps } from '@/components/Grid';
import { Roles } from '@/constants/roles';
import { GetApprovedAbsencesResponse } from '@/queries/dashboard/types';

export interface TodayAbsenceGridRow extends GridCellDefaultProps {
	userId: number;
	inquiryId: number;
	userFullName: string;
	userPosition: Roles;
	inquiryType: string;
	inquiryTemplateName: string;
}

export interface TodayAbsencesGridComponentProps {
	data: GetApprovedAbsencesResponse;
}
