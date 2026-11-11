import { ChevronIcon } from '@mms/mms-ui-library/icons/base/ChevronIcon';
import { KeyboardEvent } from 'react';

import { EVENT_ENTER_KEY } from '@/constants/eventKeys';

import { smallIconParams } from './constants';
import { ChevronButtonWrapper } from './styles';
import { ChevronButtonProps } from './types';

export function ChevronButton({
	onClick,
	size = 'small',
	...rest
}: ChevronButtonProps) {
	const handleKeyDown = (event: KeyboardEvent<HTMLSpanElement>) => {
		if (event.key === ' ' || event.key === EVENT_ENTER_KEY) {
			event.preventDefault();
			onClick?.();
		}
	};

	return (
		<ChevronButtonWrapper
			{...{ onClick, size, ...rest }}
			onKeyDown={handleKeyDown}
		>
			<ChevronIcon {...(size === 'small' ? smallIconParams : {})} />
		</ChevronButtonWrapper>
	);
}
