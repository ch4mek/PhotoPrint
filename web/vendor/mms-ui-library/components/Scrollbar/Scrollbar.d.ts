import { ScrollbarProps } from './types';
/**
 * [Bug]: \@emotion/styled has a bug with the styled component type definition, within which the Union type is used.
 *
 * To avoid this and customize the Scrollbar, use on of the following syntax:
 * @example
 * const StyledScrollbar = styled(Scrollbar)(
 * 	({ theme }) => css`
 *			color: ${theme.palette['primary']};
 *		`
 * );
 *
 * // Or this one:
 * const StyledScrollbar = styled(Scrollbar)`
 * 	color: ${({ theme }) => theme.palette['primary']};
 * `
 */
export declare function Scrollbar({ maxHeight, maxWidth, ref, ...rest }: ScrollbarProps): import("react/jsx-runtime").JSX.Element;
