import styled from '@emotion/styled';
import { Tag } from '@mms/mms-ui-library/components/Tag';
import { lightPalette } from '@mms/mms-ui-library/theme';

import { TagProps } from '@/components/Tag/types';
import { DEFAULT_TAG_HEIGHT } from '@/constants/commonStyles';

export const TagWrapper = styled(Tag)<TagProps>(
	({
		isClickable,
		backgroundColor,
		color,
		width,
		height = DEFAULT_TAG_HEIGHT,
	}) => ({
		...(backgroundColor && {
			border: 'none',

			'&, &:hover': {
				backgroundColor,
			},
		}),

		height,
		width,
		minWidth: width,
		color: color || lightPalette['text-black'],
		cursor: isClickable ? 'pointer' : 'default',
	})
);
