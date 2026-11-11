import { Dayjs } from '@/lib/dayjs';
import { InternshipResponse } from '@/types/internship';
import { InternshipFinalResult, InternshipType } from '@/types/labboard';

import { TechnologyFormItem } from './components/TechnologiesFields/types';

export enum InternshipFormFields {
	Specialization = 'internshipSpecialization',
	Type = 'internshipType',
	EngineerStage = 'internshipEngineerStage',
	FinalResult = 'internshipFinalResult',
	CompletedPoints = 'internshipCompletedPoints',
	ApprovedPoints = 'internshipApprovedPoints',
	StartDate = 'internshipStartDate',
	PlannedEndDate = 'internshipPlannedEndDate',
	Technologies = 'technologies',
}

export interface InternshipFormValues {
	id: number | null;
	internshipSpecialization: string;
	internshipType: InternshipType | null;
	internshipEngineerStage: string;
	internshipFinalResult: InternshipFinalResult | null;
	internshipCompletedPoints: string;
	internshipApprovedPoints: string;
	internshipStartDate: Dayjs | null;
	internshipPlannedEndDate: Dayjs | null;
	technologies: TechnologyFormItem[];
}

export interface UpdateInternship extends InternshipFormValues {
	userId: number;
}

export interface InternshipFormProps {
	handleClose: () => void;
	onSubmit: (
		values: UpdateInternship
	) => Promise<InternshipResponse | undefined>;
	userId: number;
	initialValues: InternshipFormValues;
}
