"use client";

import React from "react";
import {
  ConfigurableForm,
  FormFullName,
  LOGIN_CONFIGURATION,
  PageWithLayout,
} from "@shesha-io/reactjs";
import styles from "./login.module.css";

const LoginPage: PageWithLayout = () => (
  <div className={styles.page}>
    <div className={styles.card}>
      <div className={styles.brand}>
        <div className={styles.logoBox}>TS</div>
        <div>
          <div className={styles.brandName}>TicketingSystem</div>
          <div className={styles.brandSub}>Support Portal</div>
        </div>
      </div>

      <div className={styles.sheshaWrap}>
        <ConfigurableForm
          mode={"edit"}
          formId={LOGIN_CONFIGURATION as FormFullName}
        />
      </div>
    </div>
  </div>
);

export default LoginPage;