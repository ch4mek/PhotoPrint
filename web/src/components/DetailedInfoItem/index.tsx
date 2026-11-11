import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { memo, useLayoutEffect, useRef, useState } from 'react';

import { Bullet } from '@/components/Bullet';
import { TOOLTIP_FALLBACK_PLACEMENTS } from '@/components/Grid/constants';
import { useResize } from '@/hooks/useResize';

import {
	DetailedInfoItemWrapper,
	DetailedInfoName,
	DetailedInfoNameWrapper,
	DetailedInfoTime,
} from './styles';
import { DetailedInfoItemProps } from './types';
import { getDetailedInfoItemTime } from './utils';

function DetailedInfoItemInner({
	color,
	name,
	value,
	onClick,
	isActive,
}: DetailedInfoItemProps) {
	const nameRef = useRef<HTMLSpanElement>(null);
	const [width] = useResize();

	const [isTextOverflowing, setIsTextOverflowing] = useState(false);

	useLayoutEffect(() => {
		if (nameRef.current) {
			setIsTextOverflowing(
				nameRef.current.scrollWidth > nameRef.current.clientWidth
			);
		}
	}, [nameRef.current, width]);

	return (
		<Tooltip
			text={name}
			position="top"
			fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
			open={isTextOverflowing}
		>
			<DetailedInfoItemWrapper isActive={isActive} onClick={onClick}>
				<DetailedInfoNameWrapper>
					<Bullet color={color} />
					<DetailedInfoName ref={nameRef} variant="p">
						{name}
					</DetailedInfoName>
				</DetailedInfoNameWrapper>
				<DetailedInfoTime variant="p">
					{getDetailedInfoItemTime(value)}
				</DetailedInfoTime>
			</DetailedInfoItemWrapper>
		</Tooltip>
	);
}

export const DetailedInfoItem = memo(DetailedInfoItemInner);
