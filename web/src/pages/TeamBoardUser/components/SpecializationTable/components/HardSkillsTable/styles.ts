import { css, Theme } from '@emotion/react';

import { overrideGridStyles } from '@/components/Grid';
import {
	createActiveBorderColor,
	createDefaultBorderColor,
	getEvenRowBackgroundColor,
	getOddRowBackgroundColor,
} from '@/helpers/getColor';

export const overrideHardSkillsStyles = (isEven: boolean) => (theme: Theme) =>
	css`
		${overrideGridStyles(false)(theme).styles};

		overflow: hidden;

		::after {
			content: '';
			position: absolute;
			bottom: 0;
			height: 1px;
			width: 100%;
			background: ${createDefaultBorderColor(theme)};
		}

		:has(> :nth-last-child(1 of .rdg-row):hover)::after {
			background: ${createActiveBorderColor(theme)};
		}

		.rdg-row {
			background: ${isEven
				? getEvenRowBackgroundColor(theme)
				: getOddRowBackgroundColor(theme)};
		}

		.rdg-cell[role='columnheader'] {
			display: none;
		}

		.rdg-row > .rdg-cell::after {
			content: none;
		}
	`;
