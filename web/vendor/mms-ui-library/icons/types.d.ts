import { SVGProps } from 'react';
export type SVGPropsWithStrokeColor = SVGProps<SVGSVGElement> & {
    strokeColor?: string;
};
export type SVGPropsWithDisabled = SVGProps<SVGSVGElement> & {
    disabled?: boolean;
};
export type SVGPropsWithDataTestId = SVGProps<SVGSVGElement> & {
    dataTestId?: string;
};
