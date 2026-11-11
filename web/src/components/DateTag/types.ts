import { Resources } from 'i18next';

export enum TagTypes {
	Future = 'Future',
	Upcoming = 'Upcoming',
	Imminent = 'Imminent',
	OverdueMoreThanNDays = 'OverdueMoreThanNDays',
	Passed = 'Passed',
}

type DateTagSectionKey = keyof Resources['profile']['dateTag']['tagNames'];

export interface DateTagProps {
	type: TagTypes | null;
	date: string | null;
	width?: number | string;
	onClick?: VoidFunction;
}

interface ColorConfig {
	color?: string;
	backgroundColor: string;
	nameColor: DateTagSectionKey;
	descriptionColor: DateTagSectionKey;
}

interface TagView {
	color: string;
	border: string;
	backgroundColor: string;
}

export type TagColors = Record<TagTypes, ColorConfig>;
export type TagViews = Record<TagTypes, TagView>;
