import { useTheme } from '@emotion/react';
import { ChevronLeftIcon } from '@mms/mms-ui-library/icons/base/ChevronLeftIcon';
import { PropsWithChildren } from 'react';

import { Divider } from '../Divider';
import { PageTitleTypography } from '../PageTitleTypography';

import { ButtonAdd } from '@/components/ButtonAdd';

import {
	AsideWrapper,
	BackButtonLink,
	HeaderContainer,
	HeaderControlsContainer,
	TitleRowWrapper,
	TitleWrapper,
} from './styles';
import type { PageHeaderProps } from './types';

export function PageHeader({
	children,
	title,
	titleAside,
	backTo,
	addButton,
	exportComponent,
	divider = false,
	needWrapDownDesktopXS = false,
}: PropsWithChildren<PageHeaderProps>) {
	const theme = useTheme();

	const {
		onClick,
		title: addButtonTitle,
		disabled,
		showTooltip,
		tooltipMessage,
		view,
		isHidden,
	} = addButton ?? {};

	const handleAdd = () => {
		onClick?.();
	};

	return (
		<>
			<HeaderContainer
				{...{ needWrapDownDesktopXS }}
				isWithTitle={Boolean(title)}
			>
				{(title || titleAside) && (
					<TitleRowWrapper>
						{backTo && (
							<BackButtonLink to={backTo} aria-label="Back">
								<ChevronLeftIcon />
							</BackButtonLink>
						)}
						{title && (
							<TitleWrapper>
								<PageTitleTypography title={title} />
							</TitleWrapper>
						)}
						{titleAside && <AsideWrapper>{titleAside}</AsideWrapper>}
					</TitleRowWrapper>
				)}
				<HeaderControlsContainer {...{ needWrapDownDesktopXS }}>
					{children}
					{exportComponent}
					{onClick && !isHidden && (
						<ButtonAdd
							onAdd={handleAdd}
							title={addButtonTitle}
							disabled={disabled}
							showTooltip={showTooltip}
							tooltipMessage={tooltipMessage}
							view={view}
						/>
					)}
				</HeaderControlsContainer>
			</HeaderContainer>
			{divider ? <Divider margin={`${theme.spaces.xl} 0`} /> : null}
		</>
	);
}
