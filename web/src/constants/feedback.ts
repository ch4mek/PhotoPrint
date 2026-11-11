import { FeedbackType } from '@/pages/TeamBoardUser/types';
import { GREEN_TAG_COLOR, ORANGE_TAG_COLOR, RED_TAG_COLOR } from '@/theme';
import { FormAutoselectOption } from '@/types/form';

export const FEEDBACK_TYPE_TAG_WIDTH = 62;

export const FEEDBACK_TAG_COLOR_STYLES = {
	[FeedbackType.Great]: {
		backgroundColor: GREEN_TAG_COLOR,
	},
	[FeedbackType.Normal]: {
		backgroundColor: ORANGE_TAG_COLOR,
	},
	[FeedbackType.Bad]: {
		backgroundColor: RED_TAG_COLOR,
	},
};

export const feedbackAutoselectOptions: Array<FormAutoselectOption<string>> = [
	{
		id: FeedbackType.Great,
		name: FeedbackType.Great,
	},
	{
		id: FeedbackType.Normal,
		name: FeedbackType.Normal,
	},
	{
		id: FeedbackType.Bad,
		name: FeedbackType.Bad,
	},
];
