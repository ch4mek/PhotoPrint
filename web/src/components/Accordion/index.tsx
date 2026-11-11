import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { ErrorIcon } from '@mms/mms-ui-library/icons/base/ErrorIcon';
import { useEffect, useRef, useState } from 'react';

import {
	ButtonsContainer,
	CollapsibleButton,
	CollapsibleWrapper,
	Content,
	ContentWrapper,
	ErrorIconContainer,
	ExpandButton,
} from './styles';
import { CollapsibleProps } from './types';

export function Accordion({
	children,
	onClose,
	onOpen,
	label,
	hasError = false,
	allowToOpen = true,
	isOpenByDefault = false,
	defaultContentHeight = 0,
	spacingType = 'margin',
}: CollapsibleProps) {
	const theme = useTheme();
	const contentRef = useRef<HTMLDivElement | null>(null);
	const [isOpened, setIsOpened] = useState(isOpenByDefault);
	const [contentHeight, setContentHeight] = useState(defaultContentHeight);

	useEffect(() => {
		if (!isOpened) {
			setContentHeight(0);

			return;
		}

		if (!contentRef.current) {
			return;
		}

		const observer = new ResizeObserver(() => {
			if (contentRef.current) {
				setContentHeight(contentRef.current.scrollHeight);
			}
		});

		observer.observe(contentRef.current);

		setContentHeight(contentRef.current.scrollHeight);

		return () => {
			observer.disconnect();
		};
	}, [isOpened]);

	const toggleOpen = () => {
		if (!allowToOpen) {
			return;
		}

		if (isOpened) {
			setIsOpened(false);

			if (onClose) {
				onClose(label);
			}
		} else {
			setIsOpened(true);

			if (onOpen) {
				onOpen(label);
			}
		}
	};

	return (
		<CollapsibleWrapper>
			<CollapsibleButton allowToOpen={allowToOpen} onClick={toggleOpen}>
				<Typography variant="m-600">{label}</Typography>
				<ButtonsContainer>
					<ErrorIconContainer>
						{hasError && <ErrorIcon color={theme.palette['graphics-error']} />}
					</ErrorIconContainer>
					{allowToOpen && <ExpandButton isActive={isOpened} size="medium" />}
				</ButtonsContainer>
			</CollapsibleButton>
			<ContentWrapper
				height={contentHeight}
				spacingType={spacingType}
				isOpened={isOpened}
			>
				{isOpened && <Content ref={contentRef}>{children}</Content>}
			</ContentWrapper>
		</CollapsibleWrapper>
	);
}
