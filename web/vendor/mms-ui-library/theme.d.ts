export declare enum ThemeMode {
    DARK = "dark",
    LIGHT = "light"
}
export declare const spaces: {
    readonly xs: 4;
    readonly s: 8;
    readonly m: 16;
    readonly l: 24;
    readonly xl: 32;
    readonly xl2: 40;
    readonly xl3: 56;
    readonly xl4: 64;
    readonly xl5: 72;
    readonly xl6: 80;
    readonly xl7: 88;
    readonly xl8: 96;
};
export declare const typography: {
    readonly fontFamily: "'Onest', -apple-system, BlinkMacSystemFont, 'Helvetica Neue', sans-serif";
    readonly headlines: {
        readonly large: {
            readonly fontWeight: "700";
            readonly fontSize: "20px";
            readonly lineHeight: "28px";
        };
        readonly medium: {
            readonly fontWeight: "700";
            readonly fontSize: "18px";
            readonly lineHeight: "24px";
        };
        readonly small: {
            readonly fontWeight: "700";
            readonly fontSize: "16px";
            readonly lineHeight: "20px";
        };
    };
    readonly body: {
        readonly 'l-600': {
            readonly fontWeight: "600";
            readonly fontSize: "16px";
            readonly lineHeight: "20px";
        };
        readonly 'l-400': {
            readonly fontWeight: "400";
            readonly fontSize: "16px";
            readonly lineHeight: "20px";
        };
        readonly 'm-700': {
            readonly fontWeight: "700";
            readonly fontSize: "14px";
            readonly lineHeight: "18px";
        };
        readonly 'm-600': {
            readonly fontWeight: "600";
            readonly fontSize: "14px";
            readonly lineHeight: "18px";
        };
        readonly 'm-400': {
            readonly fontWeight: "400";
            readonly fontSize: "14px";
            readonly lineHeight: "18px";
        };
        readonly 's-700': {
            readonly fontWeight: "700";
            readonly fontSize: "12px";
            readonly lineHeight: "16px";
        };
        readonly 's-600': {
            readonly fontWeight: "600";
            readonly fontSize: "12px";
            readonly lineHeight: "16px";
        };
        readonly 's-400': {
            readonly fontWeight: "400";
            readonly fontSize: "12px";
            readonly lineHeight: "16px";
        };
        readonly 'xs-600': {
            readonly fontWeight: "600";
            readonly fontSize: "10px";
            readonly lineHeight: "14px";
        };
        readonly 'xs-400': {
            readonly fontWeight: "400";
            readonly fontSize: "10px";
            readonly lineHeight: "14px";
        };
        readonly 'xxs-600': {
            readonly fontWeight: "600";
            readonly fontSize: "6px";
            readonly lineHeight: "8px";
        };
    };
};
export declare const WHITE = "#FFFFFF";
export declare const BLACK = "#000000";
export declare const lightPalette: {
    readonly 'background-primary-3': string;
    readonly 'background-primary-1': string;
    readonly 'background-base-3': string;
    readonly 'background-base-2': string;
    readonly 'background-base-1': string;
    readonly 'background-white': string;
    readonly 'background-turquoise': string;
    readonly 'background-red': string;
    readonly 'background-pink': string;
    readonly 'background-purple': string;
    readonly 'background-orange': string;
    readonly 'background-green': string;
    readonly 'background-blue': string;
    readonly 'background-light-green': string;
    readonly 'hover-primary-1': string;
    readonly 'hover-base-2': string;
    readonly 'text-white': string;
    readonly 'text-black': string;
    readonly 'text-gray': string;
    readonly 'text-accent': string;
    readonly 'text-error': string;
    readonly 'text-violet': string;
    readonly 'text-turquoise': string;
    readonly 'text-red': string;
    readonly 'text-pink': string;
    readonly 'text-purple': string;
    readonly 'text-orange': string;
    readonly 'text-green': string;
    readonly 'text-blue': string;
    readonly 'text-light-green': string;
    readonly 'graphics-white': string;
    readonly 'graphics-accent': string;
    readonly 'graphics-light-gray': string;
    readonly 'graphics-gray': string;
    readonly 'graphics-primary-2': string;
    readonly 'graphics-input-stroke': string;
    readonly 'graphics-scroll': string;
    readonly 'graphics-divider-1': string;
    readonly 'graphics-divider-2': string;
    readonly 'graphics-error': string;
    readonly 'graphics-yellow': string;
    readonly 'even-stroke-base-1': string;
    readonly 'feedback-danger': string;
    readonly 'feedback-success': string;
    readonly 'feedback-info': string;
    readonly 'primary-11': "#2C2F3A";
    readonly 'primary-10': "#2C3144";
    readonly 'primary-9': "#363944";
    readonly 'primary-8': "#383D4D";
    readonly 'primary-7': "#565A69";
    readonly 'primary-6': "#71799B";
    readonly 'primary-5': "#9BA1BD";
    readonly 'primary-4': "#D1D7EA";
    readonly 'primary-3': "#E9EBF1";
    readonly 'primary-2': "#FAFAFA";
    readonly 'primary-1': "#FFFFFF";
    readonly 'accent-6': "#F17900";
    readonly 'accent-5': "#FB9400";
    readonly 'accent-4': "#F5A140";
    readonly 'accent-3': "#F9C999";
    readonly 'accent-2': "#FCDEBF";
    readonly 'accent-1': "#FEF2E6";
    readonly 'base-6': "#373737";
    readonly 'base-5': "#5F5F5F";
    readonly 'base-4': "#878787";
    readonly 'base-3': "#C3C3C3";
    readonly 'base-2': "#EBEBEB";
    readonly 'base-1': "#F3F3F3";
    readonly 'exc-popups-primary-10': "#2C3144";
    readonly 'exc-popups-primary-1': "#FFFFFF";
    readonly 'exc-gridBgLines-primary-2': "#FAFAFA";
    readonly 'exc-primaryHeader-primary-10': "#2C3144";
    readonly 'exc-primaryHeader-primary-1': "#FFFFFF";
    readonly 'exc-chip-accent-2': "#FCDEBF";
    readonly 'exc-chip-accent-1': "#FEF2E6";
    readonly 'exc-leftMenu-primary-10': "#2C3144";
    readonly 'exc-leftMenu-primary-8': "#383D4D";
    readonly 'exc-list-primary-5': "#9BA1BD";
    readonly 'exc-mainPage-primary-10': "#2C3144";
    readonly 'exc-mainPage-primary-1': "#FFFFFF";
    readonly 'exc-hint-primary-7': "#565A69";
    readonly 'exc-hint-base-6': "#373737";
    readonly 'exc-secondCalendar-base-1': "#F3F3F3";
};
export declare const darkPalette: {
    readonly 'background-primary-3': string;
    readonly 'background-primary-1': string;
    readonly 'background-base-3': string;
    readonly 'background-base-2': string;
    readonly 'background-base-1': string;
    readonly 'background-white': string;
    readonly 'background-turquoise': string;
    readonly 'background-red': string;
    readonly 'background-pink': string;
    readonly 'background-purple': string;
    readonly 'background-orange': string;
    readonly 'background-green': string;
    readonly 'background-blue': string;
    readonly 'background-light-green': string;
    readonly 'hover-primary-1': string;
    readonly 'hover-base-2': string;
    readonly 'text-white': string;
    readonly 'text-black': string;
    readonly 'text-gray': string;
    readonly 'text-accent': string;
    readonly 'text-error': string;
    readonly 'text-violet': string;
    readonly 'text-turquoise': string;
    readonly 'text-red': string;
    readonly 'text-pink': string;
    readonly 'text-purple': string;
    readonly 'text-orange': string;
    readonly 'text-green': string;
    readonly 'text-blue': string;
    readonly 'text-light-green': string;
    readonly 'graphics-white': string;
    readonly 'graphics-accent': string;
    readonly 'graphics-light-gray': string;
    readonly 'graphics-gray': string;
    readonly 'graphics-primary-2': string;
    readonly 'graphics-input-stroke': string;
    readonly 'graphics-scroll': string;
    readonly 'graphics-divider-1': string;
    readonly 'graphics-divider-2': string;
    readonly 'graphics-error': string;
    readonly 'graphics-yellow': string;
    readonly 'even-stroke-base-1': string;
    readonly 'feedback-danger': string;
    readonly 'feedback-success': string;
    readonly 'feedback-info': string;
    readonly 'primary-10': "#565A69";
    readonly 'primary-8': "#E9EBF1";
    readonly 'primary-7': "#9BA1BD";
    readonly 'primary-6': "#9BA1BD";
    readonly 'primary-4': "#565A69";
    readonly 'primary-3': "#363944";
    readonly 'primary-2': "#2C2F3A";
    readonly 'primary-1': "#383D4D";
    readonly 'accent-6': "#FB9400";
    readonly 'accent-2': "#F9C999";
    readonly 'accent-1': "#565A69";
    readonly 'base-6': "#EBEBEB";
    readonly 'base-5': "#F3F3F3";
    readonly 'base-4': "#C3C3C3";
    readonly 'base-3': "#565A69";
    readonly 'base-2': "#565A69";
    readonly 'base-1': "#363944";
    readonly 'exc-popups-primary-10': "#383D4D";
    readonly 'exc-popups-primary-1': "#2C2F3A";
    readonly 'exc-gridBgLines-primary-2': "#363944";
    readonly 'exc-primaryHeader-primary-10': "#383D4D";
    readonly 'exc-chip-accent-1': "#FCDEBF";
    readonly 'exc-leftMenu-primary-10': "#383D4D";
    readonly 'exc-leftMenu-primary-8': "#565A69";
    readonly 'exc-mainPage-primary-10': "#E9EBF1";
    readonly 'exc-mainPage-primary-1': "#2C2F3A";
    readonly 'exc-hint-primary-7': "#E9EBF1";
    readonly 'exc-secondCalendar-base-1': "#565A69";
    readonly 'primary-11': "#2C2F3A";
    readonly 'primary-9': "#363944";
    readonly 'primary-5': "#9BA1BD";
    readonly 'accent-5': "#FB9400";
    readonly 'accent-4': "#F5A140";
    readonly 'accent-3': "#F9C999";
    readonly 'exc-primaryHeader-primary-1': "#FFFFFF";
    readonly 'exc-chip-accent-2': "#FCDEBF";
    readonly 'exc-list-primary-5': "#9BA1BD";
    readonly 'exc-hint-base-6': "#373737";
};
export declare const breakpoints: {
    readonly mobile: 390;
    readonly tabletXS: 577;
    readonly tabletS: 600;
    readonly tablet: 820;
    readonly desktopXS: 1025;
    readonly desktopS: 1366;
    readonly desktop: 1536;
    readonly desktopL: 1920;
};
export declare const lightTheme: {
    readonly mode: ThemeMode.LIGHT;
    readonly typography: {
        readonly fontFamily: "'Onest', -apple-system, BlinkMacSystemFont, 'Helvetica Neue', sans-serif";
        readonly headlines: {
            readonly large: {
                readonly fontWeight: "700";
                readonly fontSize: "20px";
                readonly lineHeight: "28px";
            };
            readonly medium: {
                readonly fontWeight: "700";
                readonly fontSize: "18px";
                readonly lineHeight: "24px";
            };
            readonly small: {
                readonly fontWeight: "700";
                readonly fontSize: "16px";
                readonly lineHeight: "20px";
            };
        };
        readonly body: {
            readonly 'l-600': {
                readonly fontWeight: "600";
                readonly fontSize: "16px";
                readonly lineHeight: "20px";
            };
            readonly 'l-400': {
                readonly fontWeight: "400";
                readonly fontSize: "16px";
                readonly lineHeight: "20px";
            };
            readonly 'm-700': {
                readonly fontWeight: "700";
                readonly fontSize: "14px";
                readonly lineHeight: "18px";
            };
            readonly 'm-600': {
                readonly fontWeight: "600";
                readonly fontSize: "14px";
                readonly lineHeight: "18px";
            };
            readonly 'm-400': {
                readonly fontWeight: "400";
                readonly fontSize: "14px";
                readonly lineHeight: "18px";
            };
            readonly 's-700': {
                readonly fontWeight: "700";
                readonly fontSize: "12px";
                readonly lineHeight: "16px";
            };
            readonly 's-600': {
                readonly fontWeight: "600";
                readonly fontSize: "12px";
                readonly lineHeight: "16px";
            };
            readonly 's-400': {
                readonly fontWeight: "400";
                readonly fontSize: "12px";
                readonly lineHeight: "16px";
            };
            readonly 'xs-600': {
                readonly fontWeight: "600";
                readonly fontSize: "10px";
                readonly lineHeight: "14px";
            };
            readonly 'xs-400': {
                readonly fontWeight: "400";
                readonly fontSize: "10px";
                readonly lineHeight: "14px";
            };
            readonly 'xxs-600': {
                readonly fontWeight: "600";
                readonly fontSize: "6px";
                readonly lineHeight: "8px";
            };
        };
    };
    readonly spaces: {
        readonly xs: 4;
        readonly s: 8;
        readonly m: 16;
        readonly l: 24;
        readonly xl: 32;
        readonly xl2: 40;
        readonly xl3: 56;
        readonly xl4: 64;
        readonly xl5: 72;
        readonly xl6: 80;
        readonly xl7: 88;
        readonly xl8: 96;
    };
    readonly palette: {
        readonly 'background-primary-3': string;
        readonly 'background-primary-1': string;
        readonly 'background-base-3': string;
        readonly 'background-base-2': string;
        readonly 'background-base-1': string;
        readonly 'background-white': string;
        readonly 'background-turquoise': string;
        readonly 'background-red': string;
        readonly 'background-pink': string;
        readonly 'background-purple': string;
        readonly 'background-orange': string;
        readonly 'background-green': string;
        readonly 'background-blue': string;
        readonly 'background-light-green': string;
        readonly 'hover-primary-1': string;
        readonly 'hover-base-2': string;
        readonly 'text-white': string;
        readonly 'text-black': string;
        readonly 'text-gray': string;
        readonly 'text-accent': string;
        readonly 'text-error': string;
        readonly 'text-violet': string;
        readonly 'text-turquoise': string;
        readonly 'text-red': string;
        readonly 'text-pink': string;
        readonly 'text-purple': string;
        readonly 'text-orange': string;
        readonly 'text-green': string;
        readonly 'text-blue': string;
        readonly 'text-light-green': string;
        readonly 'graphics-white': string;
        readonly 'graphics-accent': string;
        readonly 'graphics-light-gray': string;
        readonly 'graphics-gray': string;
        readonly 'graphics-primary-2': string;
        readonly 'graphics-input-stroke': string;
        readonly 'graphics-scroll': string;
        readonly 'graphics-divider-1': string;
        readonly 'graphics-divider-2': string;
        readonly 'graphics-error': string;
        readonly 'graphics-yellow': string;
        readonly 'even-stroke-base-1': string;
        readonly 'feedback-danger': string;
        readonly 'feedback-success': string;
        readonly 'feedback-info': string;
        readonly 'primary-11': "#2C2F3A";
        readonly 'primary-10': "#2C3144";
        readonly 'primary-9': "#363944";
        readonly 'primary-8': "#383D4D";
        readonly 'primary-7': "#565A69";
        readonly 'primary-6': "#71799B";
        readonly 'primary-5': "#9BA1BD";
        readonly 'primary-4': "#D1D7EA";
        readonly 'primary-3': "#E9EBF1";
        readonly 'primary-2': "#FAFAFA";
        readonly 'primary-1': "#FFFFFF";
        readonly 'accent-6': "#F17900";
        readonly 'accent-5': "#FB9400";
        readonly 'accent-4': "#F5A140";
        readonly 'accent-3': "#F9C999";
        readonly 'accent-2': "#FCDEBF";
        readonly 'accent-1': "#FEF2E6";
        readonly 'base-6': "#373737";
        readonly 'base-5': "#5F5F5F";
        readonly 'base-4': "#878787";
        readonly 'base-3': "#C3C3C3";
        readonly 'base-2': "#EBEBEB";
        readonly 'base-1': "#F3F3F3";
        readonly 'exc-popups-primary-10': "#2C3144";
        readonly 'exc-popups-primary-1': "#FFFFFF";
        readonly 'exc-gridBgLines-primary-2': "#FAFAFA";
        readonly 'exc-primaryHeader-primary-10': "#2C3144";
        readonly 'exc-primaryHeader-primary-1': "#FFFFFF";
        readonly 'exc-chip-accent-2': "#FCDEBF";
        readonly 'exc-chip-accent-1': "#FEF2E6";
        readonly 'exc-leftMenu-primary-10': "#2C3144";
        readonly 'exc-leftMenu-primary-8': "#383D4D";
        readonly 'exc-list-primary-5': "#9BA1BD";
        readonly 'exc-mainPage-primary-10': "#2C3144";
        readonly 'exc-mainPage-primary-1': "#FFFFFF";
        readonly 'exc-hint-primary-7': "#565A69";
        readonly 'exc-hint-base-6': "#373737";
        readonly 'exc-secondCalendar-base-1': "#F3F3F3";
    };
    readonly breakpoints: {
        readonly mobile: 390;
        readonly tabletXS: 577;
        readonly tabletS: 600;
        readonly tablet: 820;
        readonly desktopXS: 1025;
        readonly desktopS: 1366;
        readonly desktop: 1536;
        readonly desktopL: 1920;
    };
};
export declare const darkTheme: {
    readonly mode: ThemeMode.DARK;
    readonly typography: {
        readonly fontFamily: "'Onest', -apple-system, BlinkMacSystemFont, 'Helvetica Neue', sans-serif";
        readonly headlines: {
            readonly large: {
                readonly fontWeight: "700";
                readonly fontSize: "20px";
                readonly lineHeight: "28px";
            };
            readonly medium: {
                readonly fontWeight: "700";
                readonly fontSize: "18px";
                readonly lineHeight: "24px";
            };
            readonly small: {
                readonly fontWeight: "700";
                readonly fontSize: "16px";
                readonly lineHeight: "20px";
            };
        };
        readonly body: {
            readonly 'l-600': {
                readonly fontWeight: "600";
                readonly fontSize: "16px";
                readonly lineHeight: "20px";
            };
            readonly 'l-400': {
                readonly fontWeight: "400";
                readonly fontSize: "16px";
                readonly lineHeight: "20px";
            };
            readonly 'm-700': {
                readonly fontWeight: "700";
                readonly fontSize: "14px";
                readonly lineHeight: "18px";
            };
            readonly 'm-600': {
                readonly fontWeight: "600";
                readonly fontSize: "14px";
                readonly lineHeight: "18px";
            };
            readonly 'm-400': {
                readonly fontWeight: "400";
                readonly fontSize: "14px";
                readonly lineHeight: "18px";
            };
            readonly 's-700': {
                readonly fontWeight: "700";
                readonly fontSize: "12px";
                readonly lineHeight: "16px";
            };
            readonly 's-600': {
                readonly fontWeight: "600";
                readonly fontSize: "12px";
                readonly lineHeight: "16px";
            };
            readonly 's-400': {
                readonly fontWeight: "400";
                readonly fontSize: "12px";
                readonly lineHeight: "16px";
            };
            readonly 'xs-600': {
                readonly fontWeight: "600";
                readonly fontSize: "10px";
                readonly lineHeight: "14px";
            };
            readonly 'xs-400': {
                readonly fontWeight: "400";
                readonly fontSize: "10px";
                readonly lineHeight: "14px";
            };
            readonly 'xxs-600': {
                readonly fontWeight: "600";
                readonly fontSize: "6px";
                readonly lineHeight: "8px";
            };
        };
    };
    readonly spaces: {
        readonly xs: 4;
        readonly s: 8;
        readonly m: 16;
        readonly l: 24;
        readonly xl: 32;
        readonly xl2: 40;
        readonly xl3: 56;
        readonly xl4: 64;
        readonly xl5: 72;
        readonly xl6: 80;
        readonly xl7: 88;
        readonly xl8: 96;
    };
    readonly palette: {
        readonly 'background-primary-3': string;
        readonly 'background-primary-1': string;
        readonly 'background-base-3': string;
        readonly 'background-base-2': string;
        readonly 'background-base-1': string;
        readonly 'background-white': string;
        readonly 'background-turquoise': string;
        readonly 'background-red': string;
        readonly 'background-pink': string;
        readonly 'background-purple': string;
        readonly 'background-orange': string;
        readonly 'background-green': string;
        readonly 'background-blue': string;
        readonly 'background-light-green': string;
        readonly 'hover-primary-1': string;
        readonly 'hover-base-2': string;
        readonly 'text-white': string;
        readonly 'text-black': string;
        readonly 'text-gray': string;
        readonly 'text-accent': string;
        readonly 'text-error': string;
        readonly 'text-violet': string;
        readonly 'text-turquoise': string;
        readonly 'text-red': string;
        readonly 'text-pink': string;
        readonly 'text-purple': string;
        readonly 'text-orange': string;
        readonly 'text-green': string;
        readonly 'text-blue': string;
        readonly 'text-light-green': string;
        readonly 'graphics-white': string;
        readonly 'graphics-accent': string;
        readonly 'graphics-light-gray': string;
        readonly 'graphics-gray': string;
        readonly 'graphics-primary-2': string;
        readonly 'graphics-input-stroke': string;
        readonly 'graphics-scroll': string;
        readonly 'graphics-divider-1': string;
        readonly 'graphics-divider-2': string;
        readonly 'graphics-error': string;
        readonly 'graphics-yellow': string;
        readonly 'even-stroke-base-1': string;
        readonly 'feedback-danger': string;
        readonly 'feedback-success': string;
        readonly 'feedback-info': string;
        readonly 'primary-10': "#565A69";
        readonly 'primary-8': "#E9EBF1";
        readonly 'primary-7': "#9BA1BD";
        readonly 'primary-6': "#9BA1BD";
        readonly 'primary-4': "#565A69";
        readonly 'primary-3': "#363944";
        readonly 'primary-2': "#2C2F3A";
        readonly 'primary-1': "#383D4D";
        readonly 'accent-6': "#FB9400";
        readonly 'accent-2': "#F9C999";
        readonly 'accent-1': "#565A69";
        readonly 'base-6': "#EBEBEB";
        readonly 'base-5': "#F3F3F3";
        readonly 'base-4': "#C3C3C3";
        readonly 'base-3': "#565A69";
        readonly 'base-2': "#565A69";
        readonly 'base-1': "#363944";
        readonly 'exc-popups-primary-10': "#383D4D";
        readonly 'exc-popups-primary-1': "#2C2F3A";
        readonly 'exc-gridBgLines-primary-2': "#363944";
        readonly 'exc-primaryHeader-primary-10': "#383D4D";
        readonly 'exc-chip-accent-1': "#FCDEBF";
        readonly 'exc-leftMenu-primary-10': "#383D4D";
        readonly 'exc-leftMenu-primary-8': "#565A69";
        readonly 'exc-mainPage-primary-10': "#E9EBF1";
        readonly 'exc-mainPage-primary-1': "#2C2F3A";
        readonly 'exc-hint-primary-7': "#E9EBF1";
        readonly 'exc-secondCalendar-base-1': "#565A69";
        readonly 'primary-11': "#2C2F3A";
        readonly 'primary-9': "#363944";
        readonly 'primary-5': "#9BA1BD";
        readonly 'accent-5': "#FB9400";
        readonly 'accent-4': "#F5A140";
        readonly 'accent-3': "#F9C999";
        readonly 'exc-primaryHeader-primary-1': "#FFFFFF";
        readonly 'exc-chip-accent-2': "#FCDEBF";
        readonly 'exc-list-primary-5': "#9BA1BD";
        readonly 'exc-hint-base-6': "#373737";
    };
    readonly breakpoints: {
        readonly mobile: 390;
        readonly tabletXS: 577;
        readonly tabletS: 600;
        readonly tablet: 820;
        readonly desktopXS: 1025;
        readonly desktopS: 1366;
        readonly desktop: 1536;
        readonly desktopL: 1920;
    };
};
