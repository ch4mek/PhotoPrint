import { ResetFilterButton } from '@mms/mms-ui-library/components/ResetFilterButton';
import { MouseEvent, useRef } from 'react';

import { FilterHeaderCellContentWrapper } from './styles';
import { HeaderCellWithDisabledFilterProps } from './types';

export const headerCellWithDisabledFilterRenderer = ({
	disabled,
	onClick,
	displayFilterButton = true,
}: HeaderCellWithDisabledFilterProps) =>
	function () {
		const handleClick = (e: MouseEvent) => {
			e.stopPropagation();

			if (!disabled) {
				onClick();
			}
		};

		const containerRef = useRef<HTMLDivElement>(null);

		const handleOnDoubleClick = (event: MouseEvent<HTMLDivElement>) => {
			containerRef.current!.focus();
			event.preventDefault();
			event.stopPropagation();
		};

		return (
			<FilterHeaderCellContentWrapper
				disabled={disabled}
				onClick={handleClick}
				onMouseDown={handleOnDoubleClick}
				ref={containerRef}
			>
				{displayFilterButton && <ResetFilterButton disabled={disabled} />}
			</FilterHeaderCellContentWrapper>
		);
	};
