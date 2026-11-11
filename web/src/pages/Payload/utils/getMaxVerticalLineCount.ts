import { spaces } from '@mms/mms-ui-library/theme';

import { RESIZABLE_ROW_LINE_HEIGHT } from '@/constants/commonStyles';

export const getMaxVerticalLineCount = (projectItemsRowHeight: number) =>
	Math.trunc((projectItemsRowHeight - spaces.s) / RESIZABLE_ROW_LINE_HEIGHT);
