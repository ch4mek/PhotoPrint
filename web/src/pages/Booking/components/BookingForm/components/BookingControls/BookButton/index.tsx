import { Button } from '@mms/mms-ui-library/components/Button';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import isEqual from 'lodash/isEqual';
import { PropsWithChildren, ReactElement, memo } from 'react';

import { BookButtonWrapper } from './styles';
import type { BookButtonInnerProps } from './types';

function BookButtonInner({
	children,
	tooltipTitle,
	disabled,
	...buttonProps
}: PropsWithChildren<BookButtonInnerProps>) {
	return (
		<Tooltip open={Boolean(tooltipTitle) && disabled} text={tooltipTitle}>
			<BookButtonWrapper>
				<Button {...{ ...buttonProps, disabled }}>
					{children as ReactElement}
				</Button>
			</BookButtonWrapper>
		</Tooltip>
	);
}

export const BookButton = memo(BookButtonInner, isEqual);
