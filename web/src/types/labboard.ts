import { ByPageResponse } from '@/queries/types';

export enum InternshipType {
	Primary = 'Primary',
	Extended = 'Extended',
	NDA = 'NDA',
}

export enum InternshipFinalResult {
	Success = 'Success',
	Fired = 'Fired',
}

interface Internship {
	id: number;
	userId: number;
	specialization: string;
	approvedPoints: number | null;
	completedPoints: number | null;
	engineerStage: number;
	startDate: string;
	plannedEndDate: string;
	type: InternshipType;
	typeOrderWeight: number;
	finalResult: InternshipFinalResult | null;
	finalResultOrderWeight: number;
}

export interface LabboardIntern {
	id: number;
	firstName: string | null;
	lastName: string | null;
	fullName: string | null;
	internship: Internship | null;
	englishLevelId: string | null;
	englishLevelName: string | null;
	englishLevelOrderWeight: number | null;
	resourceGroupId: number | null;
	resourceGroupName: string | null;
	currentLocation: string | null;
	comments: string | null;
	isActive: boolean;
}

export interface LabboardInternsResponse {
	labBoardInterns: ByPageResponse<LabboardIntern>;
}

export interface LabboardLookupUser {
	id: number;
	firstName: string;
	lastName: string;
	firstNameRu: string;
	lastNameRu: string;
}
