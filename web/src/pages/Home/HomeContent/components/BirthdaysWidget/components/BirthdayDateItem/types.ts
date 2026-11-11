import { AllBirthdaysResponseUserBirthday } from '@/types/home';

export interface BirthdayDateItemProps
	extends AllBirthdaysResponseUserBirthday {
	monthName: string;
}
