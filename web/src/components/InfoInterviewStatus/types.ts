import { InterviewStatusNameColorKey } from '@/types/interview';

export interface InterviewStatusColorGroup {
	light: string;
	dark: string;
	statuses: InterviewStatusNameColorKey[];
}
