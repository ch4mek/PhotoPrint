import { useTheme } from '@emotion/react';
import { Tag } from '@mms/mms-ui-library/components/Tag';
import { MouseEventHandler, useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { DateTagProps, TagTypes } from '@/components/DateTag/types';
import { convertDateToTagDate, getTagViews } from '@/components/DateTag/utils';

const DEFAULT_TAG_TYPE = TagTypes.OverdueMoreThanNDays;
export function DateTag({ date, type, width, onClick }: DateTagProps) {
	const { t } = useTranslation('common');
	const theme = useTheme();
	const tagViews = useMemo(() => getTagViews(theme), [theme]);
	const handleTagClickCapture: MouseEventHandler = (mouseEvent) => {
		mouseEvent.stopPropagation();
		onClick?.();
	};

	return (
		<Tag
			onClickCapture={handleTagClickCapture}
			stylesConfig={{
				cursor: onClick ? 'pointer' : 'default',
				width,
				...tagViews[type || DEFAULT_TAG_TYPE],
			}}
		>
			{date ? convertDateToTagDate(date) : t('notPlanned')}
		</Tag>
	);
}
